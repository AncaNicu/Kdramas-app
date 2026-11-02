import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActorFormComponent } from './actor-form-component';

describe('ActorFormComponent', () => {
  let component: ActorFormComponent;
  let fixture: ComponentFixture<ActorFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActorFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ActorFormComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
