import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from '../components/dashboard/dashboard.component';
import { TableComponent } from '../components/table/table.component';
import { HeaderComponent } from '../components/header/header.component';

import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { ReviewDialogComponent } from '../components/review-dialog/review-dialog.component';
import { ToArrayPipe } from '../pipes/to-array.pipe';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzPopoverModule } from 'ng-zorro-antd/popover';

@NgModule({
  declarations: [
    DashboardComponent,
    HeaderComponent,
    TableComponent,
    ReviewDialogComponent,
    ToArrayPipe
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      {
        path: '',
        component: DashboardComponent
      }
    ]),
    FormsModule,
    ReactiveFormsModule,
    NzButtonModule,
    NzPageHeaderModule,
    NzTableModule,
    NzIconModule,
    NzModalModule,
    NzFormModule,
    NzInputModule,
    NzTagModule,
    NzSpaceModule,
    NzPopoverModule,
  ],
  exports: [ToArrayPipe]
})
export class DashboardFeatureModule { }
