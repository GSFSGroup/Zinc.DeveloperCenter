import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, NavigationEnd, Router } from '@angular/router';
import { IBreadCrumb, SharedServices } from '@gsfsgroup/kr-shell-services';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
    private destroyed$ = new Subject<void>();

    public constructor(private router: Router, private activatedRoute: ActivatedRoute, private sharedServices: SharedServices) {}

    public ngOnInit(): void {
        this.router.events.pipe(takeUntil(this.destroyed$), filter((e: any) => e instanceof NavigationEnd))
            .subscribe(() => this.publishCrumbs());
        this.publishCrumbs();

        this.router.initialNavigation();

        // Replace the properties with values for your application.
        this.sharedServices.events.dispatchAppInitialized({
            regions: {
                menu: document.createElement('zn-developercenter_menu')
            },
            title: 'Developer Center',
            subtitle: 'Your one-stop-shop for all RedLine developer resources.',

            icon: {
                type: 'element',
                value: 'zinc',
                abbreviation: 'Kr',
                category: 'Developer Center'
            }
        });
    }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    private publishCrumbs(): void {
        const crumbs = this.getBreadCrumbs(this.activatedRoute.snapshot.root, '', 0);
        crumbs.forEach(crumb => this.sharedServices.breadCrumbs.addCrumb(crumb));
    }

    private getBreadCrumbs(route: ActivatedRouteSnapshot, url: string, level: number): IBreadCrumb[] {
        let crumbs: IBreadCrumb[] = [];
        const routeUrl = url + route.url.toString();
        let nextLevel = level;

        // Angular projects the parent data down to default route (path = ''), in which case `route.url.toString() === ''`.
        if (route.url.toString() !== '' && route.data.breadcrumb) {
            crumbs = [{
                ...route.data.breadcrumb,
                hash: routeUrl,
                level: level
            }];
            nextLevel += 1;
        }

        if (route.children) {
            crumbs = route.children
                .map(child => this.getBreadCrumbs(child, routeUrl + '/', nextLevel))
                .reduce((acc, current) => acc.concat(current), crumbs);
        }

        return crumbs;
    }
}
