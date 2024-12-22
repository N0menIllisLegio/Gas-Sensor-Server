import { TranslateLoader, TranslationObject } from '@ngx-translate/core';
import { Observable, of } from 'rxjs';

import * as TranslationsRU from '../../public/i18n/ru.json';
import * as TranslationsEN from '../../public/i18n/en.json';

const TRANSLATIONS: TranslationObject = {
    en: TranslationsEN,
    ru: TranslationsRU
};

export class StaticTranslationLoader implements TranslateLoader {
    public getTranslation(lang: string): Observable<TranslationObject> {
        const translation = TRANSLATIONS[lang];
        if (translation) {
            return of(translation);
        } else {
            console.error(`Unknown language: ${lang}`);
            return of({});
        }
    }
}