import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class ErrorHandlerService 
{

  public getBackendErrorMessage(error: HttpErrorResponse): string | null 
  {
    const body: unknown = error.error;

    if (typeof body === 'string' && body.trim().length > 0) 
    {
      return body;
    }

    if (typeof body !== 'object' || body === null) 
    {
      return null;
    }

    const response = body as 
    {
      message?: unknown;
      title?: unknown;
    };

    if (
      typeof response.message === 'string' &&
      response.message.trim().length > 0
    ) 
    {
      return response.message;
    }

    if (
      typeof response.title === 'string' &&
      response.title.trim().length > 0
    ) 
    {
      return response.title;
    }

    return null;
  }
}
