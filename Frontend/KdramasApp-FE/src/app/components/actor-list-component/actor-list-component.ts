import { Component, OnInit, signal } from '@angular/core';
import { ActorService } from '../../services/actor-service/actor-service';
import { Router } from '@angular/router';
import { ToastService } from '../../services/toast-service/toast-service';
import { Actor } from '../../models/actor';
import { AuthService } from '../../services/auth-service/auth-service';
import { HttpErrorResponse } from '@angular/common/http';
import { ConfirmationDialogComponent } from '../confirmation-dialog-component/confirmation-dialog-component';

@Component({
  imports: [ConfirmationDialogComponent],
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

  //actors is initialized with an empty array
  actors = signal<Actor[]>([]);
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
    this.getActors();
  }

  getActors(): void
  {
    this.loadError.set(null);

    this.actorService.getAllActors().subscribe
    ({
      next: (response) =>
      {
        this.actors.set(response);
      },
      error: () =>
      {
        this.loadError.set("Unable to load actors. Please try again later.");
      }
    });
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

        //update the list of actors by loading it again
        //witouth the one we deleted
        this.actors.update(currentActors =>
          currentActors.filter(actor => actor.actorId !== actorId)
        );
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
    })
  }
}
