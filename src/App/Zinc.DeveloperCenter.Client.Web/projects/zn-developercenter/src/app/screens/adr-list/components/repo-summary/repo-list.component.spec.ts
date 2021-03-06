import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatMenuModule } from '@angular/material/menu';
import { of } from 'rxjs';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
import { Page } from '~/models/page.interface';
import { RepositoryListComponent } from '~/models/repo.interface';
import { GitHubRepoService } from '~/shared/services/github-repo.service';

import { RepoListComponent } from './repo-list.component';

describe('RepoListComponent', () => {
    let component: RepoListComponent;
    let fixture: ComponentFixture<RepoListComponent>;
    let repoService: jasmine.SpyObj<GitHubRepoService>;
    let loadingOverlayService: jasmine.SpyObj<LoadingOverlayService>;
    const repoPage: Page<RepositoryListComponent> = {
        hasNextPage: false,
        hasPreviousPage: false,
        isFirstPage: false,
        isLastPage: false,
        items: [],
        page: 0,
        pageSize: 0,
        totalItems: 0,
        totalPages: 0
    };

    beforeEach(async () => {
        repoService = jasmine.createSpyObj<GitHubRepoService>('repoService', {
            listApps: of(repoPage)
        });
        loadingOverlayService = jasmine.createSpyObj<LoadingOverlayService>('loadingOverlayService', ['show', 'hide']);

        await TestBed.configureTestingModule({
            declarations: [RepoListComponent],
            imports: [HttpClientTestingModule, MatMenuModule],
            providers: [
                { provide: GitHubRepoService, useValue: repoService },
                { provide: LoadingOverlayService, useValue: loadingOverlayService }
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(RepoListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
