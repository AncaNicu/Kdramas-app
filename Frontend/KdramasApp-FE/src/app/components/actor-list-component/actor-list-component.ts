import { Component, OnInit, signal } from '@angular/core';
import { ActorService } from '../../services/actor-service/actor-service';
import { Router } from '@angular/router';
import { ToastService } from '../../services/toast-service/toast-service';
import { Observable } from 'rxjs';
import { Actor } from '../../models/actor';
import { AsyncPipe } from '@angular/common';
import { catchError } from 'rxjs';
import { of } from 'rxjs';
import { AuthService } from '../../services/auth-service/auth-service';

@Component({
  imports: [AsyncPipe],
  selector: 'app-actor-list-component',
  styleUrl: './actor-list-component.css',
  templateUrl: './actor-list-component.html',
})
export class ActorListComponent implements OnInit
{
  constructor(
    private actorService: ActorService,
    private authService: AuthService,
    private toast: ToastService,
    private router: Router
  ){}

  actors$!: Observable<Actor[]>;
  //check if there was an API problem
  loadError = signal<string | null>(null);
  //check if the user is an admin
  isAdmin: boolean = false;

  ngOnInit(): void
  {
    this.isAdmin = this.authService.isAdmin();
    this.getActors();
  }

  getActors(): void
  {
    this.loadError.set(null);

    this.actors$ = this.actorService.getAllActors().pipe
    (
      //handles errors from a HTTP request
      catchError(() => 
      {
        this.loadError.set("Unable to load actors. Please try again later.");
        //creates and returns an observable 
        //that emits an empty array
        return of([]);
      })
    );
  }

  viewActorDetails(actorId: number)
  {
    this.router.navigate(['/actors', actorId]);
  }

  addActor()
  {

  }

  editActor()
  {

  }

  deleteActor()
  {

  }
}
