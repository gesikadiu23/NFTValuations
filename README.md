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

## Instructions

1. On the NFTDbContext.cs Class you need to add the connection string to your database on line 14.

```
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
      // Configure your database connection here
      optionsBuilder.UseSqlServer(@"Your_Connection_String");
  }
```
In order to invoke a contract, you can use infura.io to create a free account. From here, you need to get the mainnet
endpoint in the form of: https://mainnet.infura.io/v3/YOUR_API_KEY_VALUE_IN_HEX

2. Insert your API Key on the NFTMetadataExtractor.cs class on line 27.
3. Insert the path to the unsupportedUrls.txt file on your machine on line 28.

```
        private const string infuraEndpoint = "https://mainnet.infura.io/v3/your_Key";
        private const string unsupportedUrlsFilePath = @"";
```



## Contributing
Contributions to NFT Valuations are welcome! If you find any bugs or have suggestions for improvements, please submit an issue or open a pull request.

1. Fork the repository.
2. Create your feature branch: git checkout -b feature/your-feature-name.
3. Commit your changes: git commit -am 'Add some feature'.
4. Push to the branch: git push origin feature/your-feature-name.
5. Submit a pull request
