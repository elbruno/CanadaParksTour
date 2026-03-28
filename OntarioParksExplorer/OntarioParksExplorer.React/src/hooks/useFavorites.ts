import { useState, useEffect } from 'react';

const FAVORITES_KEY = 'ontario-parks-favorites';

export const useFavorites = () => {
  const [favorites, setFavorites] = useState<Set<number>>(() => {
    const stored = localStorage.getItem(FAVORITES_KEY);
    if (stored) {
      try {
        const parsed = JSON.parse(stored);
        return new Set(parsed);
      } catch {
        return new Set();
      }
    }
    return new Set();
  });

  useEffect(() => {
    localStorage.setItem(FAVORITES_KEY, JSON.stringify(Array.from(favorites)));
  }, [favorites]);

  const addFavorite = (parkId: number) => {
    setFavorites(prev => new Set(prev).add(parkId));
  };

  const removeFavorite = (parkId: number) => {
    setFavorites(prev => {
      const newSet = new Set(prev);
      newSet.delete(parkId);
      return newSet;
    });
  };

  const isFavorite = (parkId: number): boolean => {
    return favorites.has(parkId);
  };

  const toggleFavorite = (parkId: number) => {
    if (isFavorite(parkId)) {
      removeFavorite(parkId);
    } else {
      addFavorite(parkId);
    }
  };

  return {
    favorites,
    addFavorite,
    removeFavorite,
    isFavorite,
    toggleFavorite,
  };
};
