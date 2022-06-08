import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdrListComponent } from './adr-list.component';

describe('AdrListComponent', () => {
  let component: AdrListComponent;
  let fixture: ComponentFixture<AdrListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdrListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdrListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
