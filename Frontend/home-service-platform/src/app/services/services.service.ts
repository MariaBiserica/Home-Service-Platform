import { Injectable } from '@angular/core';
import { Service } from '../interfaces/service.interface';
import {
  Observable,
  Subject,
} from 'rxjs';
import servicesData from 'src/assets/data.json';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ServicesService {
  private servicesList: Service[] = servicesData;
  servicesListSubject = new Subject<Service[]>();
  private userToken: string = '';
  private baseUrl: string = 'https://localhost:7269/api/';
  readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: 'Bearer ',
    }),
  };

  constructor(private http: HttpClient) {}

  get services(): Service[] {
    return this.servicesList;
  }

  set services(servicesToSet: any) {
    this.servicesList = servicesToSet;
    this.servicesListSubject.next(servicesToSet);
  }

  deleteService(service: Service) {
    const serviceToDelete = this.servicesList.find(
      (val) =>
        val.name == service.name && val.description == service.description
    );
    const httpOptionsAdd = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: 'Bearer ' + localStorage.getItem('token'),
      }),
    };
    const payload = 0;
    this.http.put(
      this.baseUrl + 'providers/disable/service',
      payload,
      httpOptionsAdd
    );
  }

  addNewService(newService: Service) {
    const httpOptionsAdd = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: 'Bearer ' + localStorage.getItem('token'),
      }),
    };
    const payload = {
      title: newService.name,
      typeId: 0,
      description: newService.description,
      prices: newService.price,
    };
    this.http.post(
      this.baseUrl + 'providers/add-service',
      payload,
      httpOptionsAdd
    );
  }

  updateService(initialService: Service, updatedService: Service) {
    const httpOptionsUpdate = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: 'Bearer ' + localStorage.getItem('token'),
      }),
    };
    const payload = {
      title: updatedService.name,
      typeId: 0,
      description: updatedService.description,
      prices: updatedService.price,
    };
    this.http.put(
      this.baseUrl + 'providers/update-service',
      payload,
      httpOptionsUpdate
    );
  }

  //! This is a workaround to update the JSON file with the new services list - Needs work, server doesn't allow PUT requests
  updateServicesInJsonFile() {
    const jsonFileUrl = 'assets/data.json';

    this.http.put(jsonFileUrl, this.servicesList).subscribe(
      (response) => {
        console.log('Services list updated in JSON file');
      },
      (error) => {
        console.error('Error updating services list in JSON file:', error);
      }
    );
  }
}
