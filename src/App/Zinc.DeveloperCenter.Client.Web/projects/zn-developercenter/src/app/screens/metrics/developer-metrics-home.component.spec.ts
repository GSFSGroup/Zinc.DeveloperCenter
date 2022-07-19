import { ComponentFixture, TestBed } from '@angular/core/testing';

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
