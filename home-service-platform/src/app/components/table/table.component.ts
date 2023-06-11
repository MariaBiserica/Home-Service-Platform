import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../helpers/validators';
import { Service } from 'src/app/interfaces/service.interface';
import { ServicesService } from 'src/app/services/services.service';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
})
export class TableComponent implements OnInit {

  servicesList!: Service[];
  initialService!: Service;
  form!: FormGroup;
  isVisible: boolean = false;
  isDisabled: boolean = true;
  isInEditMode: boolean = false;

  constructor(
    private servicesService: ServicesService,
    private route: ActivatedRoute, 
    private fb: FormBuilder
  ) {
    this.route.queryParams.subscribe((res: any) => {
      console.log(res);
    });

    this.servicesService.servicesListSubject.subscribe((res: any) => {
      this.servicesList = [...res];
      console.log('in subscribe ');
    });
  }

  ngOnInit(): void {
    this.servicesList = this.servicesService.services;
    console.log(this.servicesList);
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      provider: ['', [Validators.required]],
      description: ['', [Validators.required, CustomValidators.descriptionWordsCount(5)]],
      image: ['', [Validators.required, CustomValidators.imageUrl]],
      rating: [null, [Validators.required, CustomValidators.ratingRange(0, 10)]]
    });

    this.form.valueChanges.subscribe(() => {
      this.isDisabled = this.form.invalid;
    });
  }

  deleteService(movie: Service) {
    this.servicesService.deleteService(movie);
  }

  sortByRating() {
    this.servicesService.sortByRating();
  }

  editService(service: Service) {
    this.initialService = service;

    this.isInEditMode = true;
    this.form.setValue({
      name: service.name,
      provider: service.provider,
      description: service.description,
      image: service.image,
      rating: service.rating
    });

    this.showModal();
  }

  showModal(): void {
    this.isVisible = true;
  }

  getServiceFromForm(): Service {
    return {
      name: this.form.value.name,
      provider: this.form.value.provider,
      description: this.form.value.description,
      image: this.form.value.image,
      rating: this.form.value.rating
    };
  }

  handleCancel(): void {
    this.isVisible = false;
    this.form.reset();
  }

  handleOk(): void {
    if(this.isInEditMode){
      this.isInEditMode = false;
      this.servicesService.updateService(this.initialService, this.getServiceFromForm());
    }
    else{
      this.servicesService.addNewService(this.getServiceFromForm());
    }

    this.isVisible = false;
    this.form.reset();
  }
}
