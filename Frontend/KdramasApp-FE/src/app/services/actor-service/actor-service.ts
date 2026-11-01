import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ActorRequest } from "../../models/actor-request";
import { Observable } from "rxjs";
import { APIResponse } from "../../models/api-response";
import { environment } from '../../../environments/environment';
import { Actor } from "../../models/actor";

@Injectable({
  providedIn: 'root'
})

export class ActorService 
{
    constructor(private http: HttpClient){}

    addActor(request: ActorRequest): Observable<APIResponse>
    {
        return this.http.post<APIResponse>
            (`${environment.apiUrl}/api/actor/add`, request);
    }

    updateActor(id: number, request: ActorRequest): Observable<APIResponse>
    {
        return this.http.put<APIResponse>
            (`${environment.apiUrl}/api/actor/update/${id}`, request);
    }

    getActorById(id: number): Observable<Actor>
    {
        return this.http.get<Actor>
            (`${environment.apiUrl}/api/actor/get/${id}`);
    }

    getAllActors(): Observable<Actor[]>
    {
        return this.http.get<Actor[]>
            (`${environment.apiUrl}/api/actor`);
    }

    deleteActor(id: number): Observable<APIResponse>
    {
        return this.http.delete<APIResponse>
            (`${environment.apiUrl}/api/actor/delete/${id}`);
    }
}
