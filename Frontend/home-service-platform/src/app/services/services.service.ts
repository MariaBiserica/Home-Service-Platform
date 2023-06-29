import { Injectable } from '@angular/core';
import { Service } from '../interfaces/service.interface';
import { Observable, Subject, elementAt, map, of } from 'rxjs';
import servicesData from 'src/assets/data.json';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DatabaseService } from '../interfaces/database-service.interface';

@Injectable({
  providedIn: 'root',
})
export class ServicesService {
  private servicesList: Service[] = servicesData;
  servicesListSubject = new Subject<Service[]>();
  private serviceData: DatabaseService[] = [];
  private userToken: string = '';
  private baseUrl: string = 'https://localhost:7269/api/';
  readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token'),
    }),
  };

  constructor(private http: HttpClient) {
  }

  get services(): Service[] {
    return this.servicesList;
  }

  set services(servicesToSet: any) {
    this.servicesList = servicesToSet;
    this.servicesListSubject.next(servicesToSet);
  }
  // delay(ms: number)
  // {
  //   return new Promise(resolve => setTimeout(resolve,ms));
  // } 

  getService():Observable<DatabaseService[]>
  {
      // console.log('date:');
      // console.log(this.serviceData);
      // this.serviceData.forEach((data) => {
      //   this.servicesList.push({
      //     name: data.title,
      //     provider: data.provider.user.username,
      //     description: data.description,
      //     contact: data.provider.user.email,
      //     image: '',
      //     rating: data.provider.rating,
      //     price: data.prices,
      //     workingHours: data.provider.services,
      //     location: data.provider.address,
      //   });
      // });
      // return this.servicesList;
      return this.http
      .get<DatabaseService[]>(
        this.baseUrl + 'providers/get-all-services',
        this.httpOptions
      );
  }

  deleteService(service: Service) {
    const index = this.servicesList.findIndex((item) => item === service);
    if (index != -1) {
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
    const index = this.servicesList.findIndex(
      (item) => item === initialService
    );
    if (index != -1) {
      this.servicesList[index] = updatedService;
      this.servicesListSubject.next(this.servicesList);
      //this.updateServicesInJsonFile();
    }
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
