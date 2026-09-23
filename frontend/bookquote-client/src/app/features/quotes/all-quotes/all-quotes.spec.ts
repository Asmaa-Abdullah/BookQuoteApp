import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllQuotes } from './all-quotes';

describe('AllQuotes', () => {
  let component: AllQuotes;
  let fixture: ComponentFixture<AllQuotes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllQuotes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllQuotes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
