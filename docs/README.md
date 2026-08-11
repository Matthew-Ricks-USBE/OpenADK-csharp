# docs — Reference Documentation

This folder contains reference documentation for the OpenADK project. It is organized into two sub-folders:


## [JavaVersion/](./JavaVersion/)

The OpenADK was originally written in Java. The Developer's Guide in this folder was authored for that Java implementation. Although the API details are Java-specific, the conceptual content — agent lifecycle, SIF message flow, zone/ZIS configuration, and data model design patterns — is directly relevant to the C# port as well.

> **These documents are outdated and reflect the Java version of the OpenADK. They are retained for historical reference and architectural context.**

| File | Description |
|---|---|
| [`OpenADKDevelopersGuide.pdf`](./JavaVersion/OpenADKDevelopersGuide.pdf) | Primary developer reference for the Java OpenADK. Covers agent design, SIF messaging, ADK configuration, SDO usage, and provisioning. |
| [`OpenADKDevelopersGuide.doc`](./JavaVersion/OpenADKDevelopersGuide.doc) | Word source document for the above PDF. |


## [SifSpecifications/](./SifSpecifications/)

Archived PDF releases of the SIF (Schools Interoperability Framework) specification. SIF defines the data model and messaging protocol that OpenADK agents implement.

> **These specifications are provided for reference only. Newer versions of the SIF specification may be available from the [A4L Community](https://www.a4l.org/).**

| File | SIF Version | Notes |
|---|---|---|
| [`SIF_1.1_AU.pdf`](./SifSpecifications/SIF_1.1_AU.pdf) | 1.1 (Australia) | Early Australian locale specification |
| [`SIF_1.1_UK.pdf`](./SifSpecifications/SIF_1.1_UK.pdf) | 1.1 (United Kingdom) | Early United Kingdom locale specification |
| [`SIF_15r1.pdf`](./SifSpecifications/SIF_15r1.pdf) | 1.5r1 | North America |
| [`SIF_20.pdf`](./SifSpecifications/SIF_20.pdf) | 2.0 | North America |
| [`SIF_20r1.pdf`](./SifSpecifications/SIF_20r1.pdf) | 2.0r1 | North America, revision 1 |
| [`SIF_21.pdf`](./SifSpecifications/SIF_21.pdf) | 2.1 | North America |
| [`SIF_22.pdf`](./SifSpecifications/SIF_22.pdf) | 2.2 | North America |
| [`SIF_23.pdf`](./SifSpecifications/SIF_23.pdf) | 2.3 | North America |
| [`SIF_24.pdf`](./SifSpecifications/SIF_24.pdf) | 2.4 | North America |