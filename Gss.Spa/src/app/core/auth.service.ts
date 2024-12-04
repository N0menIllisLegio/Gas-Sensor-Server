import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, from } from 'rxjs';
import Keycloak from 'keycloak-js';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
    const authService = inject(AuthService);
    const authToken = authService.accessToken;

    if (!authToken)
        return next(req);

    const newReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${authToken}`),
    });

    return next(newReq);
}

@Injectable({ providedIn: 'root' })
export default class AuthService {
    private keycloak: Keycloak;

    isLoggedIn = signal<boolean>(false);

    public get accessToken() : string | undefined {
        return this.keycloak.token;
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
        }

        this.keycloak.onAuthError = () => {
            this.isLoggedIn.set(false);
            this.keycloak.clearToken();
        }

        this.keycloak.onAuthLogout = () => {
            this.isLoggedIn.set(false);
        }
    }

    initialize() {
        from(this.keycloak.init({
            onLoad: 'check-sso',
            silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`,
            pkceMethod: 'S256',
        }))
        .pipe(catchError((err, caught) => {
            console.error('Failed initialize keycloak', err);

            return caught;
        }))
        .subscribe(x => {
            this.isLoggedIn.set(x);
        });
    }

    login() {
        this.keycloak.login({
            redirectUri: location.toString()
        });
    }

    logout() {
        this.keycloak.logout({
            redirectUri: location.origin.toString()
        });

        this.keycloak.clearToken();
    }
}