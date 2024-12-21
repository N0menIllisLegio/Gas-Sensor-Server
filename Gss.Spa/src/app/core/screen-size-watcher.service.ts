import { Injectable, signal } from '@angular/core';

@Injectable({providedIn: 'root'})
export class ScreenSizeWatcherService {
    readonly isSmallScreen = signal(false);

    constructor() {
        var x = window.matchMedia("(max-width: 835px)");
        this.onBreakPoint(x);
        x.addEventListener("change", (e) => this.onBreakPoint(e));
    }

    onBreakPoint(x: { matches: boolean }) {
      this.isSmallScreen.set(x.matches)
    }
}