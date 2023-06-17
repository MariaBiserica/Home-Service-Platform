import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../helpers/validators';
import { Service } from 'src/app/interfaces/service.interface';
import { ServicesService } from 'src/app/services/services.service';
import { debounceTime } from 'rxjs';

import { NzIconService } from 'ng-zorro-antd/icon';
import { SearchOutline } from '@ant-design/icons-angular/icons';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
})
export class TableComponent implements OnInit {

  servicesList!: Service[];
  initialService!: Service;
  searchControl!: FormControl;
  form!: FormGroup;
  isVisible: boolean = false;
  isDisabled: boolean = true;
  isInEditMode: boolean = false;

  searchTerm: string = '';
  searchIcon = 'search';

  constructor(
    private servicesService: ServicesService,
    private route: ActivatedRoute, 
    private fb: FormBuilder,
    private iconService: NzIconService
  ) {
    this.route.queryParams.subscribe((res: any) => {
      console.log(res);
    });

    this.servicesService.servicesListSubject.subscribe((res: any) => {
      this.servicesList = [...res];
      console.log('in subscribe ');
    });

    this.iconService.addIcon(...[SearchOutline]);
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

    this.searchControl = new FormControl('');
    this.searchControl.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.search();
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

  search() {
    const searchText = this.searchControl.value.toLowerCase();
  
    // Perform the search filtering
    const filteredData = this.servicesList.filter(item =>
      item.name.toLowerCase().includes(searchText) ||
      item.provider.toLowerCase().includes(searchText)
    );
  
    // Update the table data
    this.servicesList = filteredData;
  }

  onSearchTermChange(): void {
    // Check if the search term is empty
    if (!this.searchTerm) {
      this.refreshData();
    }
  }
  
  refreshData(): void {
    this.servicesList = this.servicesService.services;
  }
  
}
