import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableComponent } from 'src/app/components/table/table.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    TableComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      {
        path: '',
        component: TableComponent
      }
    ])
  ]
})
export class TableFeatureModule { }
