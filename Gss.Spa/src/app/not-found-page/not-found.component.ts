import { Component } from "@angular/core";
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from "@angular/router";
import { TranslateModule } from "@ngx-translate/core";

@Component({
    selector: 'not-found',
    imports: [MatButtonModule, RouterLink, TranslateModule],
    templateUrl: './not-found.component.html'
})
export class NotFoundComponent {
}