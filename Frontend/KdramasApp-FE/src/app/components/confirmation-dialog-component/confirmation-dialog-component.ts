import { Component, input, output } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-confirmation-dialog',
  styleUrl: './confirmation-dialog-component.css',
  templateUrl: './confirmation-dialog-component.html',
})
export class ConfirmationDialogComponent 
{
  //the message to be displayed in the dialog
  //it comes from the parent
  message = input.required<string>();

  //notify the parent that the user confirmed
  confirmed = output<void>();

  //notify the parent that the user cancelled
  cancelled = output<void>();

  confirm(): void
  {
    this.confirmed.emit();
  }

  cancel(): void
  {
    this.cancelled.emit();
  }
}
