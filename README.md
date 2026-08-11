# OpenADK — C# / .NET Implementation

The OpenADK (Agent Development Kit) is an open-source SDK for building SIF (Schools Interoperability Framework) agents. This repository contains a **C# / .NET port** of the original Java-based OpenADK, maintained by the Utah State Board of Education (USBE).

> **Note:** The original OpenADK was written in Java. This repository is a C# port of that library. Some documentation in the [`docs/`](docs/README.md) folder still refers to the Java version and is retained for historical reference.


## Repository Layout

```
OpenADK-csharp/
├── src/                  # C# source code
│   ├── core/             # Core ADK framework (locale-independent)
│   ├── au/               # Australia locale data models
│   ├── uk/               # United Kingdom locale data models
│   └── us/               # United States locale data models
├── examples/             # Sample agent projects
├── test/                 # Unit tests
├── docs/                 # Reference documentation (see docs/README.md)
├── OpenADK-NET-Core.sln  # Core-only solution
├── OpenADK-NET-AU.sln    # Australia locale solution
├── OpenADK-NET-UK.sln    # United Kingdom locale solution
└── OpenADK-NET-US.sln    # United States locale solution
```


## Documentation

Additional reference materials are available in the [`docs/`](docs/README.md) folder, including the original OpenADK Developer's Guide (Java version) and archived SIF specification PDFs.


## License

This project is licensed under the Apache License 2.0. See [LICENSE-2.0.txt](LICENSE-2.0.txt) for details.

[1]: http://nvie.com/posts/a-successful-git-branching-model
[2]: https://help.github.com/articles/using-pull-requests
[3]: http://git-scm.com/downloads
[4]: http://git-scm.com/book/en/Getting-Started-Git-Basics
[5]: https://github.com/organizations/open-adk
[7]: http://git-scm.com/book/en/Git-Branching-Basic-Branching-and-Merging#Basic-Merge-Conflicts
[8]: https://github.com/open-adk/OpenADK-csharp
