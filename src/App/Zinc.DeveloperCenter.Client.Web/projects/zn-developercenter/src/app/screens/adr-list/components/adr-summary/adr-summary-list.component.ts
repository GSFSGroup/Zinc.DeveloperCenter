import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
import { Page } from '~/models/page.interface';
import { Repo } from '~/models/repo.interface';
import { AdrSummary } from '~/models/adr-summary.interface';
import { GitHubRepoService } from '~/shared/services/github-repo.service';
import { GitHubAdrService } from '~/shared/services/github-adr.service';

@Component({
    selector: 'app-adr-summary-list',
    templateUrl: './adr-summary-list.component.html',
    styleUrls: ['./adr-summary-list.component.scss']
})
export class AdrSummaryComponent implements OnInit, OnDestroy {
    @Input()
    public repoDotName!: string;
    
    // The list of ADRs for a specific repo.
    public adrs!: Page<AdrSummary>;

    private destroyed$ = new Subject<void>();

    public constructor(
        private adrService: GitHubAdrService,
        private loadingService: LoadingOverlayService
    ) { }

    public ngOnInit(): void {
        if (this.repoDotName == 'Zinc.Templates')
        {
            this.getAdrsForTemplateRepo();
        }
        // this.getAdrsForCurrentRepo(this.repoDotName);
    }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    public getAdrsForTemplateRepo(): void {
        this.loadingService.show('Loading');
        this.adrService.listTemplateAdrs()
            .pipe(takeUntil(this.destroyed$))
            .subscribe(adrs => {
                this.adrs = adrs;
                this.loadingService.hide();
            });
    }

    public getAdrsForCurrentRepo(repoDotName: string): void {
        this.loadingService.show('Loading');
        this.adrService.listAdrs(repoDotName)
            .pipe(takeUntil(this.destroyed$))
            .subscribe(adrs => {
                this.adrs = adrs;
                this.loadingService.hide();
            });
    }
}
