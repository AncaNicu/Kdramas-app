import { Injectable, signal } from '@angular/core';
import { Toast, ToastType } from '../models/toast';

@Injectable({
  providedIn: 'root'
})

export class ToastService 
{
    toast = signal<Toast | null>(null);

    //the id of the currently active timer
    private timeoutId: ReturnType<typeof setTimeout> | undefined;

    private showToast(message: string, type: ToastType): void
    {
        if(this.timeoutId !== undefined)
        {
            //cancel timer before starting a new one
            clearTimeout(this.timeoutId);
        }

        this.toast.set({ message, type});

        this.timeoutId = setTimeout(() =>
        {
            this.toast.set(null);
            this.timeoutId = undefined;
        }, 3000);
    }

    showSuccessToast(message: string): void
    {
        this.showToast(message, ToastType.success);
    }

    showErrorToast(message: string): void
    {
        this.showToast(message, ToastType.error);
    }

    showInfoToast(message: string): void
    {
        this.showToast(message, ToastType.info);
    }
}
