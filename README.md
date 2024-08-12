# Crypto Portfolio Calculator

## Project Overview
This project is a web-based application for calculating cryptocurrency portfolio values. It allows users to upload a file containing their crypto holdings and uses real-time data from the Coinlore API to calculate current portfolio values and changes. The application also implements comprehensive logging to track operations and assist in debugging.

## Time Spent
Approximately 15 hours were spent on this project, including planning, development, testing, and implementing logging features.

## Key Design Choices
1. **Separation of Concerns**: The project is divided into a backend API (ASP.NET Core) and a frontend client (Angular), allowing for independent development and easier maintenance.
2. **Service-based Architecture**: Use of services (e.g., PortfolioFileService) to encapsulate business logic and improve testability.
3. **Exception Handling**: Custom exceptions and a global exception handling middleware to provide meaningful error messages to users.
4. **Asynchronous Operations**: Utilization of async/await for file reading and API calls to improve performance and responsiveness.
5. **Comprehensive Logging**: Implementation of file-based logging using Serilog to track application events and aid in troubleshooting.

## Interesting Techniques Used
1. **File Parsing**: Custom parsing logic to handle the specific file format required for portfolio input.
2. **API Integration**: Use of HttpClient to make calls to the Coinlore API and process the responses.
3. **Real-time Updates**: Implementation of a configurable update interval to refresh portfolio values periodically.
4. **Structured Logging**: Utilization of Serilog for structured logging, allowing for easier log analysis and filtering.

## Areas for Improvement
Given more time, the following improvements could be made:
1. Add user authentication and the ability to save multiple portfolios.
2. Enhance error handling and input validation on both frontend and backend.
3. Implement more comprehensive logging and monitoring, including log aggregation and analysis tools.
4. Add support for more cryptocurrencies and additional data sources.
5. Improve the UI/UX with more interactive charts and visualizations.
6. Implement log rotation and archiving to manage log file sizes over time.

## Testing
1. To run backend tests, navigate to the server test directory and run `dotnet test`.
2. For frontend tests, in the client directory, run `ng test`.
3. For manual testing, two sample files have been provided in the main folder:
   - `import_test_data.txt`: Contains valid test data for importing a portfolio.
   - `invalid_import_test_data.txt`: Contains invalid data to test error handling.

## Logging
1. The application uses Serilog for file-based logging, with logs stored in the `Logs` directory.
2. Log files are named `portfolio-operations-YYYYMMDD.log`, with a new file created daily.
3. Log levels include Information for general operations, Warning for potential issues, and Error for exceptions and critical problems.
4. To view logs, navigate to the `Logs` directory in the project root.
5. Log entries include timestamps, log levels, and structured data for easy parsing and analysis.