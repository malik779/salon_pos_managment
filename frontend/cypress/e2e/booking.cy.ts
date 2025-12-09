describe('Booking flow', () => {
  it('shows dashboard tiles', () => {
    cy.visit('/');
    cy.contains('Salon Overview').should('exist');
  });
});
