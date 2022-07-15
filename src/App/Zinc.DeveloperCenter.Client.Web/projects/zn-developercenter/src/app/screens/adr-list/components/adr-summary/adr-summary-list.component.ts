import { Component, OnDestroy, Input } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AdrSummary } from '~/models/adr-summary.interface';
import { Page } from '~/models/page.interface';
import { GitHubAdrService } from '~/shared/services/github-adr.service';

@Component({
    selector: 'app-adr-summary-list',
    templateUrl: './adr-summary-list.component.html',
    styleUrls: ['./adr-summary-list.component.scss']
})
export class AdrSummaryComponent implements OnDestroy {
    @Input()
    public repoDotName!: string;

    @Input()
    public sortedOn = 'number';

    @Input()
    public sortAsc = true;

    // The list of ADRs for a specific repo.
    public adrs!: Page<AdrSummary>;

    // sorting variables.
    // defines current of one of 3 options of which to sort adrs:
    // last updated date, number, title.

    private destroyed$ = new Subject<void>();

    public constructor(
        private adrService: GitHubAdrService,
    ) { }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    public getAdrsForCurrentRepo(): void {
        this.adrService.listAdrs(this.repoDotName, this.sortedOn, this.sortAsc)
            .pipe(takeUntil(this.destroyed$))
            .subscribe(adrs => {
                this.adrs = adrs;
                this.updateLastUpdatedDates();
                if (this.sortedOn === 'lud') {
                    console.log("hit!");
                    if (this.sortAsc) {
                        console.log(adrs.items[0].lastUpdatedDate);
                        this.adrs.items.sort((a, b) => new Date(a.lastUpdatedDate).getTime() - new Date(b.lastUpdatedDate).getTime());
                    } else {
                        this.adrs.items.sort((a, b) => new Date(b.lastUpdatedDate).getTime() - new Date(a.lastUpdatedDate).getTime());
                    }
                }
            });
    }

    public updateLastUpdatedDates(): void {
        this.adrs.items.forEach(adr => {
            this.adrService.updateDates(this.repoDotName, adr.adrTitle)
                .subscribe(_lastUpdatedDate => {
                    adr.lastUpdatedDate = _lastUpdatedDate;
                    adr.lastUpdatedDateString = new Date(_lastUpdatedDate).toLocaleDateString(undefined, {
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                    });
                });
        });
    }

    public encodeUrl(val: string): string {
        return encodeURIComponent(val);
    }
}
