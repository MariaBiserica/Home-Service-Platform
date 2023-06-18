import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { Review } from 'src/app/interfaces/review.interface';

@Component({
  selector: 'app-review-dialog',
  templateUrl: './review-dialog.component.html',
  styleUrls: ['./review-dialog.component.scss']
})
export class ReviewDialogComponent {

  @Input() review!: Review;
  editing: boolean = false;

  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(
    private modalRef: NzModalRef,
    private fb: FormBuilder,
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
      author: ['', [Validators.required]]
    });

    this.form.valueChanges.subscribe(() => {
      this.isDisabled = this.form.invalid;
    });
  }

  closeDialog(): void {
    this.modalRef.close();
  }

  editReview(): void {
    this.editing = true;
    this.form.patchValue(this.review);
  }

  handleCancel(): void {
    this.editing = false;
    this.form.reset();
  }

  handleOk(): void {
    this.review = this.form.value;

    this.editing = false;
    this.form.reset();
  }
}
