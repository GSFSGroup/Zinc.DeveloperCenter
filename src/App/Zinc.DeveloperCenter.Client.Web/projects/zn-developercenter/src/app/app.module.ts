import { CUSTOM_ELEMENTS_SCHEMA, DoBootstrap, Injector, NgModule } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { BrowserModule } from '@angular/platform-browser';


import { AppRoutingModule } from '~/app/app-routing.module';
import { CoreModule } from '~/core/core.module';
import { EmptyComponent } from '~/modules/empty/empty.component';
import { HomeComponent } from '~/screens/home/home.component';

import { AppComponent } from './app.component';
import { MenuComponent } from './modules/menu/menu.component';
import { AutoFocusDirective } from './shared/directives/autofocus.directive';
import { AdrListComponent } from './screens/adr-list/adr-list.component';

@NgModule({
    declarations: [AppComponent, EmptyComponent, HomeComponent, MenuComponent, AutoFocusDirective, AdrListComponent],
    imports: [AppRoutingModule, BrowserModule, CoreModule],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: []
})
export class AppModule implements DoBootstrap {
    public constructor(injector: Injector) {
        customElements.define('zn-developercenter', createCustomElement(AppComponent, { injector }));
        customElements.define('zn-developercenter_menu', createCustomElement(MenuComponent, { injector }));
    }

    // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
    public ngDoBootstrap(): void {}
}
