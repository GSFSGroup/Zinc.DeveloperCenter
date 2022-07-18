import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatMenuModule } from '@angular/material/menu';
import { of } from 'rxjs';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
import { Page } from '~/models/page.interface';
import { RepositoryListComponent } from '~/models/repo.interface';
import { GitHubRepoService } from '~/shared/services/github-repo.service';
import { DeveloperMetricsHomeComponent } from './developer-metrics-home.component';

describe('DeveloperMetricsHomeComponent', () => {
    let component: DeveloperMetricsHomeComponent;
    let fixture: ComponentFixture<DeveloperMetricsHomeComponent>;
    
    beforeEach(() => {
        fixture = TestBed.createComponent(DeveloperMetricsHomeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
