import { createEntityStore } from "@app/core/state/app.store";
import { Branch } from "@app/core/models/domain.models";
import { BranchApi } from "@app/core/api/api-client";

export const BranchStore = createEntityStore<Branch>(
    BranchApi,
    branch => branch.id
  );
  