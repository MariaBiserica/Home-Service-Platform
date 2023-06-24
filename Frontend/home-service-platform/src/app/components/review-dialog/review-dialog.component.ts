import { Component, Input } from '@angular/core';
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
  isNewReview: boolean = false; // Track if it's a new review

  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(
    private modalRef: NzModalRef,
    private fb: FormBuilder,
  ) { }
  

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
    this.isNewReview = false;
    this.form.patchValue(this.review);
  }

  addReview(): void {
    this.editing = true;
    this.isNewReview = true;
    this.form.reset();
  }

  handleCancel(): void {
    this.editing = false;
    this.isNewReview = false;
    this.form.reset();
  }

  handleOk(): void {
    this.review = this.form.value;
  
    this.editing = false;
    this.isNewReview = false;
    this.form.reset();
  }
  
}
