<template>
  <Suspense>
    <template #default>
      <to-do />
    </template>
    <template #fallback>
      <span>Loading...</span>
    </template>
  </Suspense>
</template>

<script>
import ToDo from '@/components/ToDo';
export default {
  name: 'App',
  components: {ToDo}

}
</script>
