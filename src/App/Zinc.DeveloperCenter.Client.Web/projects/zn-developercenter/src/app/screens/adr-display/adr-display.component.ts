import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { IBreadCrumb, SharedServices } from '@gsfsgroup/kr-shell-services';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { GitHubAdrService } from '~/shared/services/github-adr.service';

@Component({
    selector: 'app-adr-display',
    templateUrl: './adr-display.component.html',
    styleUrls: ['./adr-display.component.scss']
})
export class AdrDisplayComponent implements OnInit {
    public adrTitle!: string;
    public adrNumberString!: string;
    public applicationName!: string;
    public filePath!: string;
    public contentUrl!: string;

    public adrContent!: string;

    private destroyed$ = new Subject<void>();

    public constructor(
        private activatedRoute: ActivatedRoute,
        private sharedServices: SharedServices,
        private adrService: GitHubAdrService,
        private router: Router) {}

    public ngOnInit(): void {
        this.activatedRoute.params.pipe(takeUntil(this.destroyed$)).subscribe(params => {
            this.adrTitle = params.adrTitle;
            this.adrNumberString = params.adrNumberString;
            this.adrNumberString = this.adrNumberString?.toUpperCase();
            this.applicationName = params.applicationName;
            this.filePath = decodeURIComponent(params.filePath);
            this.publishCrumbs();
        });

        this.downloadAdr();
    }

    private downloadAdr(): void {
        this.adrService.downloadAdrContent(this.applicationName, this.filePath)
            .pipe(takeUntil(this.destroyed$))
            .subscribe(content => {
                this.adrContent = content.content;
                this.contentUrl = content.contentUrl;
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

    public encodeUrl(val: string): string {
        return encodeURIComponent(val);
    }
}
