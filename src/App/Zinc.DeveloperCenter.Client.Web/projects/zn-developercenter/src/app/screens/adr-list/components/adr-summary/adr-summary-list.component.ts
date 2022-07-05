import { Component, OnDestroy, Input, SimpleChanges } from '@angular/core';
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
export class AdrSummaryComponent implements OnDestroy {
    @Input()
    public repoDotName!: string;
    
    // The list of ADRs for a specific repo.
    public adrs!: Page<AdrSummary>;

    private destroyed$ = new Subject<void>();

    public constructor(
        private adrService: GitHubAdrService,
        private loadingService: LoadingOverlayService
    ) { }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    public getAdrsForCurrentRepo(): void {
        console.log("getting adrs");
        this.adrService.listAdrs(this.repoDotName)
            .pipe(takeUntil(this.destroyed$))
            .subscribe(adrs => {
                this.adrs = adrs;
            });
    }
}
