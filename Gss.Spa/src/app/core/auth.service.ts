import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, from, mergeMap } from 'rxjs';
import Keycloak from 'keycloak-js';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
    const authService = inject(AuthService);

    return from(authService.initializationPromise)
        .pipe(
            mergeMap(() => {
                const authToken = authService.accessToken;

                if (!authToken)
                    return next(req);

                const newReq = req.clone({
                    headers: req.headers.set('Authorization', `Bearer ${authToken}`),
                });

                return next(newReq);
            })
        );
}

@Injectable({ providedIn: 'root' })
export default class AuthService {
    private keycloak: Keycloak;
    private resolveInitialization: ((value: boolean | PromiseLike<boolean>) => void) | undefined = undefined;

    isLoggedIn = signal<boolean>(false);
    isAuthOperationInProgress = signal<boolean>(true); // silent sso
    initializationPromise = new Promise<boolean>((res) => {
        this.resolveInitialization = res;
    });

    public get accessToken() : string | undefined {
        return this.keycloak.token;
    }

    public get userId() : string | undefined {
        return this.keycloak.subject;
    }

    public get isAdmin() : boolean | undefined {
        return (this.keycloak.tokenParsed as { roles: string[] })?.roles.includes('Administrator');
    }

    constructor() {
        this.keycloak = new Keycloak({
            url: "http://localhost:18080",
            realm: "gas-sensor-server",
            clientId: "spa-client"
        });

        this.keycloak.onTokenExpired = async () => {
            try {
                const isRefreshed = await this.keycloak.updateToken(30);

                this.isLoggedIn.set(isRefreshed);
            } catch (error) {
                this.isLoggedIn.set(false);
                this.keycloak.clearToken();
                console.error('Failed to refresh token:', error);
            }
        }

        this.keycloak.onAuthSuccess = () => {
            this.isLoggedIn.set(true);
            this.isAuthOperationInProgress.set(false);
        }

        this.keycloak.onAuthError = () => {
            this.isLoggedIn.set(false);
            this.isAuthOperationInProgress.set(false);
            this.keycloak.clearToken();
        }

        this.keycloak.onAuthLogout = () => {
            this.isLoggedIn.set(false);
        }

        from(this.keycloak.init({
            onLoad: 'check-sso',
            silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`,
            pkceMethod: 'S256',
        }))
        .pipe(catchError((err) => {
            console.error('Failed initialize keycloak', err);

            this.resolveInitialization!(false);

            throw err;
        }))
        .subscribe(x => {
            this.isLoggedIn.set(x);
            this.isAuthOperationInProgress.set(false);

            this.resolveInitialization!(x);
        });
    }

    login() {
        this.isAuthOperationInProgress.set(true);
        this.keycloak.login({
            redirectUri: location.toString()
        });
    }

    logout() {
        this.isAuthOperationInProgress.set(true);

        this.keycloak.logout();

        this.keycloak.clearToken();

        this.isAuthOperationInProgress.set(false);
    }
}