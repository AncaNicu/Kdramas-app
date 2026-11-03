import { Component, OnInit, signal } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { ActorService } from '../../services/actor-service/actor-service';
import { AuthService } from '../../services/auth-service/auth-service';
import { Observable, catchError, EMPTY } from 'rxjs';
import { ActorDetails } from '../../models/actor-details';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  imports: [AsyncPipe],
  selector: 'app-actor-details-component',
  styleUrl: './actor-details-component.css',
  templateUrl: './actor-details-component.html',
})
export class ActorDetailsComponent  implements OnInit
{
  constructor(
    private actorService: ActorService,
    private authService: AuthService,
    //to be able to retrieve the actor's id
    private route: ActivatedRoute
  ){}

  actorDetails$!: Observable<ActorDetails>;
  //check if there was an API problem
  loadError = signal<string | null>(null);
  //check if the user is an admin
  isAdmin: boolean = false;

  ngOnInit(): void
  {
    this.isAdmin = this.authService.isAdmin();

    //get the actor's id from the route
    const id = Number(this.route.snapshot.paramMap.get("id"));

    //for an invalid id, load the error
    if(!Number.isInteger(id) || id <= 0)
    {
      this.loadError.set("Invalid actor id.");
      return;
    }

    //call the function to display the details for the current actor
    this.getActorDetails(id);
  }

  getActorDetails(id: number): void
  {
    this.loadError.set(null);
    
    this.actorDetails$ = this.actorService.getActorById(id).pipe
    (
      //handles errors from a HTTP request
      catchError((error: HttpErrorResponse) =>
      {
        console.log('HTTP error status:', error.status);
        console.log('HTTP error response:', error.error);
        if(error.status === 404)
        {
          this.loadError.set("Actor not found.");
        }
        else
        {
          this.loadError.set("Unable to load actor details. Please try again later.");
        }
        //creates and returns an observable 
        //that emits an empty object
        return EMPTY;
      })
    );
  }
}
