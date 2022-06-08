import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { EmptyComponent } from './empty.component';

describe('EmptyComponent', () => {
    let component: EmptyComponent;
    let fixture: ComponentFixture<EmptyComponent>;

    beforeEach(
        waitForAsync(() => {
            TestBed.configureTestingModule({
                declarations: [EmptyComponent],
                providers: [SharedServices]
            }).compileComponents();
        })
    );

    beforeEach(() => {
        fixture = TestBed.createComponent(EmptyComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
