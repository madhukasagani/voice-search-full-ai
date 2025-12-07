# Voice Search - Full AI Version (Starter)

This repository is a starter **Full AI Voice Search** project built for e-commerce.
It includes:

- ASP.NET 8 backend (API) with services for embeddings (OpenAI), vector search (Qdrant), and optional Azure STT.
- Simple frontend (static HTML) demonstrating client STT and audio upload.
- Docker Compose configuration to run Qdrant and the backend.
- Scripts and placeholders to index product embeddings.

**IMPORTANT:** This starter contains working code skeletons and implementation examples, but some parts
(OpenAI keys, Azure STT integration, full audio stream adapters) require you to add real credentials
and possibly small adaptations before production use.

## Quick start (local)

1. Set environment variables or create a `.env` file:
   - OPENAI_API_KEY=your_openai_key
   - QDRANT_URL=http://localhost:6333
   - AZURE_SPEECH_KEY=your_azure_key (optional)
   - AZURE_SPEECH_REGION=your_azure_region (optional)

2. Build & run with Docker Compose (requires Docker):
   ```bash
   docker compose up --build
   ```

3. Backend will be available on http://localhost:5000 and frontend is `frontend/index.html`.

## What you get in this ZIP
- backend/VoiceSearch.Api/ (C# .NET 8 Web API minimal project files)
- frontend/index.html (demo UI)
- docker-compose.yml (Qdrant + backend)
- scripts/index_products.sh (example indexing script)

## Next steps
- Replace placeholder API keys.
- Index your product catalog using the provided indexing script or console tool.
- Improve audio conversion (ffmpeg) and push audio into STT stream adapters.
- Add authentication, rate limiting, logging, and monitoring for production.
