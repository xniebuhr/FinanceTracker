import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgForm } from '@angular/forms';
import { TransactionService } from '../../services/transaction.service';
import { Transaction } from '../../models/transaction.model'

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './transaction-form.component.html'
})
export class TransactionFormComponent {
  transaction: Transaction = {
    id: 0,
    date: '',
    category: '',
    amount: 0,
    type: '',
    description: ''
  };

  constructor(private transactionService: TransactionService) {}

  onSubmit(form: NgForm): void {
    // Call the API to add the transaction
    this.transactionService.addTransaction(this.transaction).subscribe({
      next: (newTransaction) => {
        console.log('Transaction added:', newTransaction);
        // Reset the form after submission
        form.resetForm();
      },
      error: (err) => console.error(err)
    });
  }
}