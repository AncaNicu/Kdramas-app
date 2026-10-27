import { Component } from '@angular/core';
import { ToastService } from '../../services/toast-service';

@Component({
  imports: [],
  selector: 'app-toast',
  styleUrl: './toast-component.css',
  templateUrl: './toast-component.html',
})
export class ToastComponent 
{
  constructor(public toastService: ToastService) {}
}
