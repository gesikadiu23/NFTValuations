# NFT Valuations

NFT Valuations is a project that extracts metadata for Non-Fungible Tokens (NFTs) and stores the extracted data in a database. It provides functionality to process a dictionary of NFTs, extract their metadata, and insert the extracted data into a database.

## Features

- Extracts metadata for a dictionary of NFTs by contract address and token index.
- Caches the extracted metadata to improve performance.
- Checks if an NFT already exists in the database before insertion.
- Compares the properties of an NFT to determine if they have changed.
- Concatenates contract address and token index to create a unique identifier.
- Inserts the extracted metadata into a database in batches.

## Requirements

- .NET Core SDK [3.1]
- Mictosoft.EntityFrameworkCore [3.1.32]
- Mictosoft.EntityFrameworkCore.SqlServer [3.1.32]
- Mictosoft.EntityFrameworkCore.Tools [3.1.32]

## Installation

1. Clone the repository:
git clone [https://github.com/gesikadiu23/NFTValuations.git]


## Contributing
Contributions to NFT Valuations are welcome! If you find any bugs or have suggestions for improvements, please submit an issue or open a pull request.

1. Fork the repository.
2. Create your feature branch: git checkout -b feature/your-feature-name.
3. Commit your changes: git commit -am 'Add some feature'.
4. Push to the branch: git push origin feature/your-feature-name.
5. Submit a pull request
