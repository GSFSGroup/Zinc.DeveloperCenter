import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, DoBootstrap, Injector, NgModule } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { BrowserModule } from '@angular/platform-browser';
import { MarkdownModule } from 'ngx-markdown';


import { AppRoutingModule } from '~/app/app-routing.module';
import { CoreModule } from '~/core/core.module';
import { EmptyComponent } from '~/modules/empty/empty.component';
import { HomeComponent } from '~/screens/home/home.component';

import { AppComponent } from './app.component';
import { MenuComponent } from './modules/menu/menu.component';
import { AdrDisplayComponent } from './screens/adr-display/adr-display.component';
import { AdrSummaryComponent } from './screens/adr-list/components/adr-summary/adr-summary-list.component';
import { RepoListComponent } from './screens/adr-list/components/repo-summary/repo-list.component';
import { DeveloperMetricsHomeComponent } from './screens/metrics/developer-metrics-home.component';
import { AutoFocusDirective } from './shared/directives/autofocus.directive';

@NgModule({
    declarations: [AppComponent, EmptyComponent, HomeComponent, MenuComponent, AutoFocusDirective, RepoListComponent, AdrSummaryComponent, AdrDisplayComponent, DeveloperMetricsHomeComponent],
    imports: [AppRoutingModule, BrowserModule, CoreModule, HttpClientModule, MarkdownModule.forRoot({ loader: HttpClient })],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: []
})
export class AppModule implements DoBootstrap {
    public constructor(injector: Injector) {
        customElements.define('zn-developercenter', createCustomElement(AppComponent, { injector }));
        customElements.define('zn-developercenter_menu', createCustomElement(MenuComponent, { injector }));
    }

    // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
    public ngDoBootstrap(): void { }
}
