//types of toasts
export enum ToastType
{
    success = 'success',
    error = 'error',
    info = 'info'
}

export interface Toast
{
    message: string;
    type: ToastType;
}
