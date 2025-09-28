cd /d "finance_tracker.backend\finance_tracker.backend"
start cmd /k dotnet run --launch-profile "https"

cd /d "..\finance_tracker.frontend"
start cmd /k ng serve

start "" "http://localhost:4200/"
