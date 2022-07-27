import { Component, OnDestroy, Input, OnChanges } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AdrSummary } from '~/models/adr-summary.interface';
import { Page } from '~/models/page.interface';
import { GitHubAdrService } from '~/shared/services/github-adr.service';

@Component({
    selector: 'app-adr-summary-list',
    templateUrl: './adr-summary-list.component.html',
    styleUrls: ['./adr-summary-list.component.scss']
})
export class AdrSummaryComponent implements OnChanges, OnDestroy {
    @Input()
    public applicationName!: string;

    @Input()
    public sortedOn = 'number';

    @Input()
    public sortAsc = true;

    @Input()
    public expanded = false;

    // The list of ADRs for a specific repo.
    public adrs!: Page<AdrSummary>;

    // sorting variables.
    // defines current of one of 3 options of which to sort adrs:
    // last updated date, number, title.

    private destroyed$ = new Subject<void>();

    public constructor(
        private adrService: GitHubAdrService
    ) { }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    public ngOnChanges(): void {
        this.getAppAdrs();
    }

    private getAppAdrs(): void {
        if (this.expanded) {
            this.adrService.listAppAdrs(this.applicationName)
                .pipe(takeUntil(this.destroyed$))
                .subscribe(adrs => {
                    this.adrs = adrs;
                    console.log(this.adrs.items[0].numberVal);
                    console.log(this.adrs.items[0].numberDisplay);
                    console.log(this.adrs.items[1].numberVal);
                    console.log(this.adrs.items[1].numberDisplay);
                });
        }
    }

    public encodeUrl(val: string): string {
        return encodeURIComponent(val);
    }
}
