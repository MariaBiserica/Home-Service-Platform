import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../helpers/validators';
import { Service } from 'src/app/interfaces/service.interface';
import { ServicesService } from 'src/app/services/services.service';
import { debounceTime } from 'rxjs';

import { NzTableSortFn, NzTableSortOrder } from 'ng-zorro-antd/table';
import { NzIconService } from 'ng-zorro-antd/icon';
import { SearchOutline } from '@ant-design/icons-angular/icons';
import { NzModalService } from 'ng-zorro-antd/modal';
import { ReviewDialogComponent } from '../review-dialog/review-dialog.component';
import { Review } from 'src/app/interfaces/review.interface';
import { User } from 'src/app/interfaces/user.interface';
import { UserService } from 'src/app/services/user.service';

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

  user: User = {
    firstName: '',
    lastName: '',
    email: '',
    username: '',
    password: '',
    imageUrl: '',
    role: '',
  };

  // Sorting variables for table header
  serviceNameSortFn: NzTableSortFn<Service> = (a: Service, b: Service) => {
    return a.name.localeCompare(b.name);
  };
  serviceNameSortOrder: NzTableSortOrder | null = null;

  providerSortFn: NzTableSortFn<Service> = (a: Service, b: Service) => {
    return a.provider.localeCompare(b.provider);
  };
  providerSortOrder: NzTableSortOrder | null = null;

  descriptionSortFn: NzTableSortFn<Service> = (a: Service, b: Service) => {
    return a.description.localeCompare(b.description);
  };
  descriptionSortOrder: NzTableSortOrder | null = null;

  contactSortFn: NzTableSortFn<Service> = (a: Service, b: Service) => {
    return a.contact.localeCompare(b.contact);
  };
  contactSortOrder: NzTableSortOrder | null = null;

  ratingSortFn: NzTableSortFn<Service> = (a: Service, b: Service) => {
    return a.rating - b.rating;
  };  
  ratingSortOrder: NzTableSortOrder | null = null;

  constructor(
    private servicesService: ServicesService,
    private route: ActivatedRoute, 
    private fb: FormBuilder,
    private iconService: NzIconService,
    private modalService: NzModalService,
    private router: Router,
    private userService: UserService,
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
    this.user = this.userService.getCurrentUser();

    this.servicesList = this.servicesService.services;
    console.log(this.servicesList);
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      provider: ['', [Validators.required]],
      description: ['', [Validators.required, CustomValidators.descriptionWordsCount(5)]],
      contact: ['', [Validators.required, CustomValidators.emailOrPhone]],
      image: ['', [Validators.required, CustomValidators.imageUrl]],
      rating: [null, [Validators.required, CustomValidators.ratingRange(0, 5)]]
    });

    this.form.valueChanges.subscribe(() => {
      this.isDisabled = this.form.invalid;
    });

    this.searchControl = new FormControl('');
    this.searchControl.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.search();
    });
  }

  // Services functions
  deleteService(movie: Service) {
    this.servicesService.deleteService(movie);
  }

  editService(service: Service) {
    this.initialService = service;

    this.isInEditMode = true;
    this.form.setValue({
      name: service.name,
      provider: service.provider,
      description: service.description,
      contact: service.contact,
      image: service.image,
      rating: service.rating
    });

    this.showModal();
  }

  // Check before logout functions
  confirmLogout(): void {
    const confirmed = window.confirm('Are you sure you want to disconnect?');
    if (confirmed) {
      this.router.navigate(['/login']);
    }
  }

  // Modal functions
  showModal(): void {
    this.isVisible = true;
  }

  getServiceFromForm(): Service {
    return {
      name: this.form.value.name,
      provider: this.form.value.provider,
      description: this.form.value.description,
      contact: this.form.value.contact,
      review: this.form.value.review,
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

  // Open review dialog table action
  openReviewDialog(service: Service, review: Review | undefined): void {
    this.initialService = service;
    const modalRef = this.modalService.create({
      nzTitle: 'Review',
      nzContent: ReviewDialogComponent,
      nzComponentParams: {
        review: review
      }
    });
  
    this.modalService.afterAllClose.subscribe(() => {
      if (modalRef.getContentComponent().review) {
        // Handle the updated review if needed
        service.review = modalRef.getContentComponent().review;
        this.servicesService.updateService(this.initialService, service);
      }
    });
  } 

  // Search bar functions
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
  
  // Sorting table by header click functions
  sortTable(column: string): void {
    if (column === 'name') {
      const sortedList = this.servicesList.sort((a, b) =>
        this.serviceNameSortFn(a, b) * (this.serviceNameSortOrder === 'ascend' ? 1 : -1)
      );
      this.servicesList = [...sortedList]; // Assign the sorted data to a new array 
      this.serviceNameSortOrder = this.serviceNameSortOrder === 'ascend' ? 'descend' : 'ascend';
    }
    else if (column === 'provider') {
      const sortedList = this.servicesList.sort((a, b) =>
        this.providerSortFn(a, b) * (this.providerSortOrder === 'ascend' ? 1 : -1)
      );
      this.servicesList = [...sortedList]; // Assign the sorted data to a new array
      this.providerSortOrder = this.providerSortOrder === 'ascend' ? 'descend' : 'ascend';
    }
    else if (column === 'description') {
      const sortedList = this.servicesList.sort((a, b) =>
        this.descriptionSortFn(a, b) * (this.descriptionSortOrder === 'ascend' ? 1 : -1)
      );
      this.servicesList = [...sortedList]; // Assign the sorted data to a new array
      this.descriptionSortOrder = this.descriptionSortOrder === 'ascend' ? 'descend' : 'ascend';
    }
    else if (column === 'contact') {
      const sortedList = this.servicesList.sort((a, b) =>
        this.contactSortFn(a, b) * (this.contactSortOrder === 'ascend' ? 1 : -1)
      );
      this.servicesList = [...sortedList]; // Assign the sorted data to a new array
      this.contactSortOrder = this.contactSortOrder === 'ascend' ? 'descend' : 'ascend';
    }
    else if (column === 'rating') {
      const sortedList = this.servicesList.sort((a, b) =>
        this.ratingSortFn(a, b) * (this.ratingSortOrder === 'ascend' ? 1 : -1)
      );
      this.servicesList = [...sortedList]; // Assign the sorted data to a new array
      this.ratingSortOrder = this.ratingSortOrder === 'ascend' ? 'descend' : 'ascend';
    }
  }  
  
  getSortIcon(column: string): string {
    if (column === 'name') {
      if (this.serviceNameSortOrder === 'ascend') {
        return 'arrow-up';
      } else if (this.serviceNameSortOrder === 'descend') {
        return 'arrow-down';
      }
    }
    else if (column === 'provider') {
      if (this.providerSortOrder === 'ascend') {
        return 'arrow-up';
      } else if (this.providerSortOrder === 'descend') {
        return 'arrow-down';
      }
    }
    else if (column === 'description') {
      if (this.descriptionSortOrder === 'ascend') {
        return 'arrow-up';
      } else if (this.descriptionSortOrder === 'descend') {
        return 'arrow-down';
      }
    }
    else if (column === 'contact') {
      if (this.contactSortOrder === 'ascend') {
        return 'arrow-up';
      } else if (this.contactSortOrder === 'descend') {
        return 'arrow-down';
      }
    }
    else if (column === 'rating') {
      if (this.ratingSortOrder === 'ascend') {
        return 'arrow-up';
      } else if (this.ratingSortOrder === 'descend') {
        return 'arrow-down';
      }
    }
    return '';
  } 
}
