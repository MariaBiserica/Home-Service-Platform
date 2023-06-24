import { Injectable } from '@angular/core';
import { Service } from '../interfaces/service.interface';
import { Subject } from 'rxjs';
import servicesData from 'src/assets/data.json';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ServicesService {

  private servicesList: Service[] = servicesData;
  servicesListSubject = new Subject<Service[]>();

  constructor(private http: HttpClient) {}

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
      this.updateServicesInJsonFile();
      this.servicesListSubject.next(this.servicesList);
    }
  }

  addNewService(newService: Service) {
    this.servicesList.push(newService);
    this.updateServicesInJsonFile();
    this.servicesListSubject.next(this.servicesList);
  }

  updateService(initialService: Service, updatedService: Service) {
    const index = this.servicesList.findIndex((item) => item === initialService);
    if(index != -1){
      this.servicesList[index] = updatedService;
      this.servicesListSubject.next(this.servicesList);
      //this.updateServicesInJsonFile();
    }
  }

  //! This is a workaround to update the JSON file with the new services list - Needs work, server doesn't allow PUT requests
  updateServicesInJsonFile() {
    const jsonFileUrl = 'assets/data.json';
  
    this.http.put(jsonFileUrl, this.servicesList)
      .subscribe(
        response => {
          console.log('Services list updated in JSON file');
        },
        error => {
          console.error('Error updating services list in JSON file:', error);
        }
      );
  }

}
