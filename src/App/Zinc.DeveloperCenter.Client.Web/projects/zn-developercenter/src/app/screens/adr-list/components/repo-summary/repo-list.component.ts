import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
import { Page } from '~/models/page.interface';
import { RepositoryListComponent } from '~/models/repo.interface';
import { GitHubRepoService } from '~/shared/services/github-repo.service';

@Component({
    selector: 'app-repo-list',
    templateUrl: './repo-list.component.html',
    styleUrls: ['./repo-list.component.scss']
})
export class RepoListComponent implements OnInit, OnDestroy {
    public repos!: Page<RepositoryListComponent>;
    public fetchedRepos = false;

    public sortedOn = 'number';
    public sortAsc = true;

    private destroyed$ = new Subject<void>();

    public constructor(
        private repoService: GitHubRepoService,
        private loadingService: LoadingOverlayService
    ) { }

    public ngOnInit(): void {
        this.getGSFSGitHubReposInit();
    }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    public getGSFSGitHubReposInit(): void {
        this.loadingService.show('Loading');
        this.repoService.listRepos()
            .pipe(takeUntil(this.destroyed$))
            .subscribe(repos => {
                this.repos = repos;
                this.loadingService.hide();
                this.fetchedRepos = true;
            });
    }

    // sort functions will sort Adrs on one of three options: date, number, or title,
    // or they will swap asc/desc.

    public sortByLastUpdatedDate(): void {
        if (this.sortedOn === 'lud') {
            this.sortAsc = !this.sortAsc;
        } else {
            this.sortAsc = true;
            this.sortedOn = 'lud';
        }
    }

    public sortByNumber(): void {
        if (this.sortedOn === 'number') {
            this.sortAsc = !this.sortAsc;
        } else {
            this.sortAsc = true;
            this.sortedOn = 'number';
        }
    }

    public sortByTitle(): void {
        if (this.sortedOn === 'title') {
            this.sortAsc = !this.sortAsc;
        } else {
            this.sortAsc = true;
            this.sortedOn = 'title';
        }
    }
}
