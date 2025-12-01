import { Component, OnInit, ChangeDetectorRef } from '@angular/core';

import { TransactionService } from '../../services/transaction.service';
import { Transaction } from '../../models/transaction.model'

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [],
  templateUrl: './transaction-list.component.html',
  styleUrls: ['./transaction-list.component.css']
})
export class TransactionListComponent implements OnInit {
  transactions: Transaction[] = [];
  showList = false;
  hasLoaded = false;

  constructor(
    private transactionService: TransactionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Don't auto-load transactions
  }

  toggleList(): void {
    this.showList = !this.showList;
    this.hasLoaded = false;
    this.transactions = [];
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.transactionService.getTransactions().subscribe({
      next: (data) => {
        this.transactions = data,
        this.hasLoaded = true;
        this.cdr.detectChanges();
        console.log(this.hasLoaded);
        console.log(this.transactions);
      },
      error: (err) => {
        console.error(err)
        this.hasLoaded = false;
      }
    });
  }

  deleteTransaction(id: number): void {
    this.transactionService.deleteTransaction(id).subscribe({
      next: () => {
        console.log('Transaction deleted:', id);
        this.loadTransactions(); // Refresh the list
      },
      error: (err) => console.error(err)
    });
  }
}