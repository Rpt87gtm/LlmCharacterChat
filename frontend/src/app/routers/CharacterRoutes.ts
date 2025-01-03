import CharacterList from "@/pages/character/ui/CharacterList.vue";
import CharacterDetails from "@/pages/character/ui/CharacterDetails.vue";
import CharacterCreate from "@/pages/character/ui/CharacterCreate.vue";

export const CharacterRoutes = [
  {
    path: "/characters",
    name: "CharacterList",
    component: CharacterList,
    meta: { requiresAuth: true },
  },
  {
    path: "/characters/create",
    name: "CharacterCreate",
    component: CharacterCreate,
    meta: { requiresAuth: true },
  },
  {
    path: "/characters/:id",
    name: "CharacterDetails",
    component: CharacterDetails,
    meta: { requiresAuth: true },
  },
];
