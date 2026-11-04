import { Component, OnInit, signal } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { ActorService } from '../../services/actor-service/actor-service';
import { AuthService } from '../../services/auth-service/auth-service';
import { Observable, catchError, EMPTY } from 'rxjs';
import { ActorDetails } from '../../models/actor-details';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastService } from '../../services/toast-service/toast-service';
import { ConfirmationDialogComponent } from '../confirmation-dialog-component/confirmation-dialog-component';

@Component({
  imports: [AsyncPipe, ConfirmationDialogComponent],
  selector: 'app-actor-details-component',
  styleUrl: './actor-details-component.css',
  templateUrl: './actor-details-component.html',
})
export class ActorDetailsComponent  implements OnInit
{
  constructor(
    private actorService: ActorService,
    private authService: AuthService,
    private toast: ToastService,
    //to be able to retrieve the actor's id
    private route: ActivatedRoute, 
    private router: Router
  ){}

  actorDetails$!: Observable<ActorDetails>;
  //check if there was an API problem
  loadError = signal<string | null>(null);
  //check if the user is an admin
  isAdmin: boolean = false;

  //confirmation dialog related stuff --> DELETE
  //controls the visisbility of the dialog
  showDeleteConfirmation = signal(false);
  //stores the id of the actor that has to be deleted
  selectedActorId = signal<number| null>(null);

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

  //open the confirmation dialog
  openDeleteConfirmationDialog(actorId: number): void
  {
    this.selectedActorId.set(actorId);
    this.showDeleteConfirmation.set(true);
  }

  //used when the user clicks on Cancel
  cancelDelete(): void
  {
    this.selectedActorId.set(null);
    this.showDeleteConfirmation.set(false);
  }

  //used when the user confirms the deletion
  confirmDelete(): void
  {
    const actorId = this.selectedActorId();

    if(actorId === null)
    {
      return;
    }

    this.actorService.deleteActor(actorId).subscribe
    ({
      next: (response) =>
      {
        this.toast.showSuccessToast(response.message);
        this.router.navigate(['/actors']);

        //closes the dialog
        this.cancelDelete();
      },
      error: (error: HttpErrorResponse) =>
      {
        this.toast.showErrorToast(
          error.error?.message ?? "Unable to delete actor. Please try again later."
        );

        //closes the dialog
        this.cancelDelete();
      }
    });
  }
}
