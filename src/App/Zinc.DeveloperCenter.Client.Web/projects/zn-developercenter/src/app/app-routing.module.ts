import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EmptyComponent } from '~/modules/empty/empty.component';
import { HomeComponent } from '~/screens/home/home.component';

import { AdrDisplayComponent } from './screens/adr-display/adr-display.component';
import { RepoListComponent } from './screens/adr-list/components/repo-summary/repo-list.component';
import { DeveloperMetricsHomeComponent } from './screens/metrics/developer-metrics-home.component';

/**
 *   Define routes in this section. It is important that the routes of this application
 * are children to the top-level route `developercenter`, which is registered in the shell config for this app.
 *   Wildcard `**` route using the EmptyComponent is a must-have. It prevents the application's router
 * from trying to meddle in the shell's router. If you remove it, users might not be able to switch to
 * another app.
 *   Define breadcrumb for each route in the data section. They will be set in order from root to the matched route.
 * Sample route definition could be:
 * ```typescript
 * [
    {
        path: 'developercenter',
        data: {
            // Level 0 breadcrumb
            breadcrumb: {
                title: 'App - level 0',
                description: 'Description for zn-developercenter'
            }
        },
        children: [
            // TODO: Add your routes here.
            // No breadcrumb for empty routes.
            { path: '', component: HomeComponent },
            {
                path: 'datasets',
                data: {
                    // Level of breadcrumb is the depth in the routing definition.
                    breadcrumb: {
                        title: 'Datasets list - level 1',
                        description: ''
                    }
                },
                children: [
                    // No breadcrumb for empty routes.
                    { path: '', component: DatasetListComponent },
                    {
                        path: ':id',
                        data: {
                            // Level of breadcrumb is the depth in the routing definition.
                            breadcrumb: {
                                title: 'Dataset detail - level 2',
                                description: 'One with name in zn-developercenter'
                            }
                        },
                        component: DatasetDetailComponent
                    }
                ]
            },
            { path: '**', redirectTo: '' }
        ]
    },
    { path: '**', component: EmptyComponent }
   ]
 * ```
 * In this sample, if the user is in `#/developercenter/datasets/some-dataset-id` route, the breadcrumbs will be
 * `RedLine > App - level 0 > Datasets list - level 1 > Dataset detail - level 2`.
 */
export const routes: Routes = [
    {
        path: 'developer-center',
        data: {
            // Add breadcrumb data
            breadcrumb: {
                title: 'Developer Center',
                description: 'Your one-stop-shop for all RedLine developer resources.'
            }
        },
        children: [
            // TODO: Add your routes here..
            { path: '', component: HomeComponent }, // No breadcrumb for empty routes.
            {
                path: 'redline-adrs',
                data: {
                    breadcrumb: {
                        title: 'RedLine ADRs',
                        description: 'Search for and view RedLine architectural decision records.'
                    }
                },
                children: [
                    { path: '', component: RepoListComponent},
                    {
                        path: 'adr-render/:adrTitle/:adrNumberString/:applicationName/download-url/:downloadUrl/html-url/:htmlUrl',
                        data: {
                            breadcrumb: {
                                title: 'ADR Render',
                                description: 'This data should be passed in.'
                            }
                        },
                        component: AdrDisplayComponent
                    }
                ]
            },
            {
                path: 'developer-metrics',
                data: {
                    breadcrumb: {
                        title: 'Developer Metrics',
                        description: 'Keep a close eye on RedLine\'s health and progress.'
                    }
                },
                component: DeveloperMetricsHomeComponent
            },
            { path: '**', redirectTo: '' }
        ]
    },
    // This catch-all route allows switching to another microapp.
    { path: '**', component: EmptyComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true, relativeLinkResolution: 'legacy'})],
    exports: [RouterModule]
})
export class AppRoutingModule {}
