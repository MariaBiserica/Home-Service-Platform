import { Injectable } from '@angular/core';
import { Service } from '../interfaces/service.interface';
import { Subject } from 'rxjs';
import servicesData from './data.json';

@Injectable({
  providedIn: 'root'
})
export class ServicesService {

  private servicesList: Service[] = servicesData;
  servicesListSubject = new Subject<Service[]>();

  constructor() {}

  get services(): Service[] {
    return this.servicesList;
  }

  set services(servicesToSet: any) {
    this.servicesList = servicesToSet;
    this.servicesListSubject.next(servicesToSet);
  }

  deleteService(service: Service) {
    const index = this.servicesList.findIndex((item) => item === service);
    if(index != -1){
      this.servicesList.splice(index, 1);
      this.servicesListSubject.next(this.servicesList);
    }
  }

  addNewService(newService: Service) {
    this.servicesList.push(newService);
    this.servicesListSubject.next(this.servicesList);
  }

  updateService(initialMovie: Service, movie: Service) {
    const index = this.servicesList.findIndex((item) => item === initialMovie);
    if(index != -1){
      this.servicesList[index] = movie;
      this.servicesListSubject.next(this.servicesList);
    }
  }

}
