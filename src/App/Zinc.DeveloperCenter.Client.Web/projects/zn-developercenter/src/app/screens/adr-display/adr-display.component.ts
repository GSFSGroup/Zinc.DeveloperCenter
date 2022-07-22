import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { IBreadCrumb, SharedServices } from '@gsfsgroup/kr-shell-services';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'app-adr-display',
    templateUrl: './adr-display.component.html',
    styleUrls: ['./adr-display.component.scss']
})
export class AdrDisplayComponent implements OnInit {
    public adrTitle!: string;
    public adrNumberString!: string;
    public applicationName!: string;
    public downloadUrl!: string;
    public htmlUrl!: string;

    private destroyed$ = new Subject<void>();

    public constructor(
        private activatedRoute: ActivatedRoute,
        private sharedServices: SharedServices,
        private router: Router) {}

    public ngOnInit(): void {
        this.activatedRoute.params.pipe(takeUntil(this.destroyed$)).subscribe(params => {
            this.adrTitle = params.adrTitle;
            this.adrNumberString = params.adrNumberString;
            this.adrNumberString = this.adrNumberString?.toUpperCase();
            this.applicationName = params.applicationName;
            this.downloadUrl = decodeURIComponent(params.downloadUrl);
            this.htmlUrl = decodeURIComponent(params.htmlUrl);
            this.publishCrumbs();
        });
    }

    private publishCrumbs(): void {
        const crumbs = this.getBreadCrumbs(this.activatedRoute.snapshot.root, '', 0);

        if (crumbs.length > 0) {
            const crumb = crumbs[crumbs.length - 1];

            crumb.title = this.adrTitle;
            crumb.description = this.applicationName.concat(' - ', this.adrNumberString);

            crumbs.forEach(c => this.sharedServices.breadCrumbs.addCrumb(c));
        }
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
